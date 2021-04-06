import React, { useRef, useEffect } from "react"
import { css } from "@emotion/react"
import { CodeContentPair as AdminLogPair } from "./admin"
import { CodeContentPair as ApiLogPair } from "./api"
import { CodeContentPair as LogPair } from "./log"
import { Switch } from "../content"
import { Code, DefaultCodePath, Menu } from "./menu"

export { Code } from "./menu"

const pairs = () => [AdminLogPair, ApiLogPair, LogPair]

export const Content = ({ codes, path = DefaultCodePath, ...props }) => {
    return <Switch path={path} pairs={pairs()} />
}

export const codeMenuContentPair = [Code, [Menu, Content]]
