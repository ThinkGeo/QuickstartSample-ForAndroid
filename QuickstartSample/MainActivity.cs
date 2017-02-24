using Android.App;
using Android.OS;
using System.IO;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace QuickstartSample
{
    [Activity(Label = "QuickstartSample", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            string targetDirectory = (@"/mnt/sdcard/Android.Sample/GetStarted/");
            CopySampleData(targetDirectory);

            MapView mapView = FindViewById<MapView>(Resource.Id.MapView);
            mapView.MapUnit = GeographyUnit.DecimalDegree;

            WorldStreetsAndImageryOverlay worldStreetsAndImageryOverlay = new WorldStreetsAndImageryOverlay();
            mapView.Overlays.Add("WorldMapKit", worldStreetsAndImageryOverlay);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(Path.Combine(targetDirectory, "cntry02.shp"));

            // Set the worldLayer with a preset Style, as AreaStyles.Country1 has YellowGreen background and black border, our worldLayer will have the same render style.  
            AreaStyle areaStyle = new AreaStyle();
            areaStyle.FillSolidBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 233, 232, 214));
            areaStyle.OutlinePen = new GeoPen(GeoColor.FromArgb(255, 118, 138, 69), 1);
            areaStyle.OutlinePen.DashStyle = LineDashStyle.Solid;
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = areaStyle;
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            ShapeFileFeatureLayer capitalLayer = new ShapeFileFeatureLayer(Path.Combine(targetDirectory, "capital.shp"));
            // We can customize our own Style. Here we pass in a color and size.
            capitalLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.White, 7, GeoColor.StandardColors.Brown);
            // The Style we set here is available from ZoomLevel01 to ZoomLevel05. That means if we zoom in a bit more, it will no longer be visible.
            capitalLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level05;

            // Similarly, we use the presetPointStyle for cities.     
            PointStyle pointStyle = new PointStyle();
            pointStyle.SymbolType = PointSymbolType.Square;
            pointStyle.SymbolSolidBrush = new GeoSolidBrush(GeoColor.StandardColors.White);
            pointStyle.SymbolPen = new GeoPen(GeoColor.StandardColors.Black, 1);
            pointStyle.SymbolSize = 6;

            PointStyle stackStyle = new PointStyle();
            stackStyle.SymbolType = PointSymbolType.Square;
            stackStyle.SymbolSolidBrush = new GeoSolidBrush(GeoColor.StandardColors.Maroon);
            stackStyle.SymbolPen = new GeoPen(GeoColor.StandardColors.Transparent, 0);
            stackStyle.SymbolSize = 2;

            pointStyle.CustomPointStyles.Add(stackStyle);

            capitalLayer.ZoomLevelSet.ZoomLevel06.DefaultPointStyle = pointStyle;
            // The Style we set here is available from ZoomLevel06 to ZoomLevel20. That means if we zoom out a bit more, it will no longer be visible.
            capitalLayer.ZoomLevelSet.ZoomLevel06.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            ShapeFileFeatureLayer capitalLabelLayer = new ShapeFileFeatureLayer(Path.Combine(targetDirectory, "capital.shp"));
            // We can customize our own TextStyle. Here we pass in the font, size, style and color.
            capitalLabelLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("CITY_NAME", "Arial", 8, DrawingFontStyles.Italic, GeoColor.StandardColors.Black, 3, 3);
            // The TextStyle we set here is available from ZoomLevel01 to ZoomLevel05. 
            capitalLabelLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level05;

            // We use the preset TextStyle. Here we pass in "CITY_NAME", the name of the field containing the values we want to label the map with.
            GeoFont font = new GeoFont("Arial", 9, DrawingFontStyles.Bold);
            GeoSolidBrush txtBrush = new GeoSolidBrush(GeoColor.StandardColors.Maroon);
            TextStyle textStyle = new TextStyle("CITY_NAME", font, txtBrush);
            textStyle.XOffsetInPixel = 0;
            textStyle.YOffsetInPixel = -6;
            capitalLabelLayer.ZoomLevelSet.ZoomLevel06.DefaultTextStyle = textStyle;
            // The TextStyle we set here is available from ZoomLevel06 to ZoomLevel20. 
            capitalLabelLayer.ZoomLevelSet.ZoomLevel06.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay overlay = new LayerOverlay();
            overlay.Opacity = 0.8;
            overlay.Layers.Add(worldLayer);
            overlay.Layers.Add(capitalLayer);
            // Add the label layer to LayerOverlay.
            overlay.Layers.Add(capitalLabelLayer);

            mapView.Overlays.Add("Countries02", overlay);

            mapView.CurrentExtent = new RectangleShape(5, 78, 30, 26);
        }

        private void CopySampleData(string targetDirectory)
        {
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
                foreach (string filename in Assets.List("AppData"))
                {
                    Stream stream = Assets.Open("AppData/" + filename);
                    FileStream fileStream = File.Create(Path.Combine(targetDirectory, filename));
                    stream.CopyTo(fileStream);
                    fileStream.Close();
                    stream.Close();
                }
            }
        }
    }
}

