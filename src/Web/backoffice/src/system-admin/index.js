import React, { useRef, useEffect } from "react"
import { CodeContentPair as LanguagePair } from "./language"
import { CodeContentPair as CurrencyPair } from "./currency"
import { CodeContentPair as BetLimitPair } from "./betlimit"
import { CodeContentPair as SettingsPair } from "./setting"
import { Switch } from "../content"
import { Code, DefaultCodePath, Menu } from "./menu"

export { Code } from "./menu"

const pairs = () => [LanguagePair, CurrencyPair, BetLimitPair, SettingsPair]

export const Content = ({ codes, path = DefaultCodePath, ...props }) => (
    <Switch path={path} pairs={pairs()} />
)

export const codeMenuContentPair = [Code, [Menu, Content]]
