import React, { useRef, useEffect } from "react"
import { css } from "@emotion/react"
import { CodeContentPair as AgentListPair } from "./agent-list"
import { CodeContentPair as AdjustmentListPair } from "./adjustment"
import { Switch } from "../content"
import { Code, DefaultCodePath, Menu } from "./menu"

export { Code } from "./menu"

const pairs = () => [AgentListPair, AdjustmentListPair]

export const Content = ({ codes, path = DefaultCodePath, ...props }) => (
    <Switch path={path} pairs={pairs()} />
)

export const codeMenuContentPair = [Code, [Menu, Content]]
