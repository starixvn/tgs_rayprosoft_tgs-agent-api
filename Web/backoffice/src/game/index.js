import React, { useRef, useEffect } from "react"
import { CodeContentPair as GamePair } from "./games"
import { CodeContentPair as GameProviderPair } from "./providers"
import { Switch } from "../content"
import { Code, DefaultCodePath, Menu } from "./menu"

export { Code } from "./menu"

const pairs = () => [GamePair, GameProviderPair]

export const Content = ({ codes, path = DefaultCodePath, ...props }) => (
  <Switch path={path} pairs={pairs()} />
)

export const codeMenuContentPair = [Code, [Menu, Content]]
