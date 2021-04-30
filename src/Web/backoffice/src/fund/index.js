import React, { useRef, useEffect } from "react"
import { css } from "@emotion/react"
import { CodeContentPair as DepositListPair } from "./deposit"
import { CodeContentPair as WithdrawListPair } from "./withdraw"
import { Switch } from "../content"
import { Code, DefaultCodePath, Menu } from "./menu"

export { Code } from "./menu"

const pairs = () => [DepositListPair, WithdrawListPair]

export const Content = ({ codes, path = DefaultCodePath, ...props }) => (
    <Switch path={path} pairs={pairs()} />
)

export const codeMenuContentPair = [Code, [Menu, Content]]
