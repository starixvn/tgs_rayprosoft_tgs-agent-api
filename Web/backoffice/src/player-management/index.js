import React, { useRef, useEffect } from "react"
import { css } from "@emotion/core"
import { CodeContentPair as PlayerListPair } from "./player-list"
import { Switch } from "../content"
import { Code, DefaultCodePath, Menu } from "./menu"

export { Code } from "./menu"

const pairs = () => [PlayerListPair]

export const Content = ({ codes, path = DefaultCodePath, ...props }) => (
  <Switch path={path} pairs={pairs()} />
)

export const codeMenuContentPair = [Code, [Menu, Content]]
