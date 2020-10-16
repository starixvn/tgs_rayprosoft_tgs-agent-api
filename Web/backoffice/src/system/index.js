import React, { useRef, useEffect } from "react"
import { css } from "@emotion/core"
import { CodeContentPair as AdminPair } from "./admins"
import { CodeContentPair as GroupPair } from "./roles"
import { CodeContentPair as DepartmentPair } from "./departments"
import { Switch } from "../content"
import { Code, DefaultCodePath, Menu } from "./menu"

export { Code } from "./menu"

const pairs = () => [AdminPair, GroupPair, DepartmentPair]

export const Content = ({ codes, path = DefaultCodePath, ...props }) => (
  <Switch path={path} pairs={pairs()} />
)

export const codeMenuContentPair = [Code, [Menu, Content]]
