import React, { useRef, useEffect } from "react"
import { css } from "@emotion/react"
import { CodeContentPair as MarketReportPair } from "./market-report"
import { CodeContentPair as OrderReportPair } from "./order-report"
import { CodeContentPair as VipReportPair } from "./vip-report"
import { Switch } from "../content"
import { Code, DefaultCodePath, Menu } from "./menu"

export { Code } from "./menu"

const pairs = () => [MarketReportPair, OrderReportPair, VipReportPair]

export const Content = ({ codes, path = DefaultCodePath, ...props }) => (
    <Switch path={path} pairs={pairs()} />
)

export const codeMenuContentPair = [Code, [Menu, Content]]
