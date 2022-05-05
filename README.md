# MexcTriangularArbitrage

## 概要

MEXCの市場データを取得し、三角アービトラージの検知、実行をする。
以下のいずれかの順でのみ検知します。

* USDT -> BTC -> 何らかの通貨 -> USDT
* USDT -> ETH -> 何らかの通貨 -> USDT

**実際にトレードを行う場合、バグや売買不成立などにより損失が出る可能性があります。**

**本ツールの使用、及びその結果は、すべて自己責任でお願いします。**

## ビルド

対象フレームワーク: .NET 5.0

```console
cd path/to/repository
dotnet build
```

## 使い方

### 準備（前提条件）

* MEXCのアクセストークンを取得していること
* 口座に20USDTを超えるUSDTが入金されていること

### コマンド

```dos
MexcTriangularArbitrage trade
```

実際にトレードを行う。
処理は20USDTごとに処理を行われる。

**実際にトレードを行う場合、バグや売買不成立などにより損失が出る可能性があります。**

**本ツールの使用、及びその結果は、すべて自己責任でお願いします。**

```dos
MexcTriangularArbitrage simurate
```

シミュレーションを行う。
実際に売買しない都合上、実取引よりも良い結果が出る傾向にある。

* 約定しないケースが存在しない
* 取引が活発でない通貨については、板が更新されずに繰り返し対象になってしまう
  * シミュレーションであり実際に取引しない（＝板が更新されない）ことが原因

```dos
MexcTriangularArbitrage config
```

現在のアクセストークン関係の設定を確認する。

```dos
MexcTriangularArbitrage config mode=recreate
```

アクセストークンの設定を再作成する。


詳しくは、ヘルプの出力結果を参照してください。

```dos
> MexcTriangularArbitrage --help
MexcTriangularArbitrage:
  Mexc triangular arbitrage executor / simurator

Usage:
  MexcTriangularArbitrage [options] [command]

Options:
  --version    Display version information

Commands:
  trade
  simurate
  config
```
