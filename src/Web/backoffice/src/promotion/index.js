import React, { useRef, useEffect } from "react"
import { CodeContentPair as PromotionPair } from "./promotion-list"
import { CodeContentPair as PayoutPair } from "./payout-list"
import { Switch } from "../content"
import { Code, DefaultCodePath, Menu } from "./menu"

export { Code } from "./menu"

const pairs = () => [PromotionPair, PayoutPair]

export const Content = ({ codes, path = DefaultCodePath, ...props }) => (
    <Switch path={path} pairs={pairs()} />
)

export const codeMenuContentPair = [Code, [Menu, Content]]
