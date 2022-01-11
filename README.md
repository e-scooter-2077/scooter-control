# Scooter Control
[![Continuous Integration](https://github.com/e-scooter-2077/scooter-control/actions/workflows/ci.yml/badge.svg?event=push)](https://github.com/e-scooter-2077/scooter-control/actions/workflows/ci.yml)
[![GitHub issues](https://img.shields.io/github/issues-raw/e-scooter-2077/scooter-control?style=plastic)](https://github.com/e-scooter-2077/scooter-control/issues)
[![GitHub pull requests](https://img.shields.io/github/issues-pr-raw/e-scooter-2077/scooter-control?style=plastic)](https://github.com/e-scooter-2077/scooter-control/pulls)
[![GitHub](https://img.shields.io/github/license/e-scooter-2077/scooter-control?style=plastic)](/LICENSE)
[![GitHub release (latest SemVer including pre-releases)](https://img.shields.io/github/v/release/e-scooter-2077/scooter-control?include_prereleases&style=plastic)](https://github.com/e-scooter-2077/scooter-control/releases)

The Scooter Control is responsible of catching and handling events that are sent to update the IotHub service by Azure.

Scooter Control manages events that concern telemetries (coming from EventGrid) or desired properties (coming from ServiceBus).

Scooter Control is implemented as an Azure Function (FaaS) triggered by events that updates IotHub.
