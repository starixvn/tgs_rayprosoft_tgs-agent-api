import React, { useRef, useEffect } from "react"
import { CodeContentPair as GameProviderPair } from "./providers"
import { CodeContentPair as BrandGamePair } from "./brandgames"
import { CodeContentPair as BrandGameProviderPair } from "./brandproviders"
import { Switch } from "../content"
import { Code, DefaultCodePath, Menu } from "./menu"

export { Code } from "./menu"

const pairs = () => [GameProviderPair, BrandGamePair, BrandGameProviderPair]

export const Content = ({ codes, path = DefaultCodePath, ...props }) => (
  <Switch path={path} pairs={pairs()} />
)

export const codeMenuContentPair = [Code, [Menu, Content]]
