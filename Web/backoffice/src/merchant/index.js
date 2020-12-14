import React, { useRef, useEffect } from "react"
import { css } from "@emotion/react"
import { Code, Menu, DefaultPaths } from "./menu"
import { Switch } from "../content"
import { Code as CompanyCode, Content as CompanyContent } from "./companies"
import { Code as PlatformCode, Content as PlatformContent } from "./platforms"

export { Code }

const pairs = () => [
  [CompanyCode, CompanyContent],
  [PlatformCode, PlatformContent]
]

export const Content = ({ codes, path = DefaultPaths, ...props }) => (
  <Switch path={path} pairs={pairs()} />
)

export const codeMenuContentPair = [Code, [Menu, Content]]
