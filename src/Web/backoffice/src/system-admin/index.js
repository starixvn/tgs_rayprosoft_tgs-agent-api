import React, { useRef, useEffect } from "react"
import { CodeContentPair as SettingsPair } from "./setting"
import { Switch } from "../content"
import { Code, DefaultCodePath, Menu } from "./menu"

export { Code } from "./menu"

const pairs = () => [SettingsPair]

export const Content = ({ codes, path = DefaultCodePath, ...props }) => (
    <Switch path={path} pairs={pairs()} />
)

export const codeMenuContentPair = [Code, [Menu, Content]]
