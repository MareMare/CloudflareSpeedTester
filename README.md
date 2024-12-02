# Cloudflare SpeedTester

Simple Speed Test CLI via **[speed.cloudflare.com](https://speed.cloudflare.com/)**.

![DEMO](docs/Demo.gif)

## Usage
```ps1
.\CloudflareSpeedTester.exe [OPTIONS]
```

## Options

| Option                    | Default  | Description                                                                                   |
|---------------------------|----------|-----------------------------------------------------------------------------------------------|
| `-h, --help`              |          | Prints help information                                                                       |
| `--display-metadata`      | True     | Displays speed test metadata                                                                 |
| `--display-summary`       | True     | Displays speed test summary                                                                  |
| `--display-json`          |          | Displays speed test results in JSON format                                                   |
| `--output-csv <PATH>`     |          | Specifies the file path to output speed test results in CSV format                           |
| `--output-json <PATH>`    |          | Specifies the file path to output speed test results in JSON format                          |
| `--force-new`             |          | Flag to overwrite existing files or create new ones for the specified CsvFilePath or JsonFilePath |

## Examples

```ps1
.\CloudflareSpeedTester.exe --output-csv sampling.csv
```

## Acknowledgments
- [CsvHelper](https://github.com/JoshClose/CsvHelper): Library for reading and writing CSV files.
- [MinVer](https://github.com/adamralph/minver): Tool for semantic versioning.
- [Spectre.Console](https://github.com/spectreconsole/spectre.console): Library for creating beautiful console applications.
- [Tech Icons](https://techicons.dev/icons/cloudflare): Source of project icons.
