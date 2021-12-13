# Pikapikachu.CrystalReport.Client

# 水晶報表產生工具

執行bin/exe檔案即有說明。

```shell
# make : 製作報表檔

  -r, --ReportName        Required. 報表名稱(可包含後綴，或不包含) e.g. aa.rpt

  -o, --OutputPath        輸出路徑(包含xml , pdf)的主路徑 , 不輸入即輸出在當前資料夾temp底下 e.g.

  -f, --FileName          輸出的檔案名稱，不輸入即使用yyyyMMdd_HHmmss

  -j, --JsonFilePath      讀取Json檔案的路徑,可為txt或json格式，與[-t]擇一。

  -t, --JsonFileText      Json String, 為 escape字符，與[-j]擇一

  -l, --loadRptPath       報表Source路徑, 不輸入默認讀取./Source

  -u, --UseFactoryMode    使用工廠模式生產

  -d, --isDebugMode       開發者模式


# clear : 清除報表檔
.\Pikapikachu.CrystalReport.Client.exe clear

# source : 製作XML來源文件
.\Pikapikachu.CrystalReport.Client.exe source -j D:\crystalreport\ecan\src\tempjson.txt
```

# Dependency

- CR13SP30MSI32_0-10010309.MSI

