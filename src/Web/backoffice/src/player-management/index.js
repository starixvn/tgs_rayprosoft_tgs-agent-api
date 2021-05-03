import React, { useRef, useEffect } from "react"
import { css } from "@emotion/react"
import { CodeContentPair as PlayerListPair } from "./player-list"
import { CodeContentPair as BalanceAdjustmentPair } from "./balance-adjustment"
import { Switch } from "../content"
import { Code, DefaultCodePath, Menu } from "./menu"

export { Code } from "./menu"

const pairs = () => [PlayerListPair, BalanceAdjustmentPair]

export const Content = ({ codes, path = DefaultCodePath, ...props }) => (
    <Switch path={path} pairs={pairs()} />
)

export const codeMenuContentPair = [Code, [Menu, Content]]
