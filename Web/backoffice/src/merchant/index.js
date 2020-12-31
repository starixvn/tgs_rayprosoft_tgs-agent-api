import React, { useRef, useEffect } from "react"
import { css } from "@emotion/react"
import { Code, Menu, DefaultPaths } from "./menu"
import { Switch } from "../content"
import { Code as ResellerCode, Content as ResellerContent } from "./resellers"
import { Code as LicenseeCode, Content as LicenseeContent } from "./licensees"
import { Code as BrandCode, Content as BrandContent } from "./brands"

export { Code }

const pairs = () => [
  [ResellerCode, ResellerContent],
  [LicenseeCode, LicenseeContent],
  [BrandCode, BrandContent]
]

export const Content = ({ codes, path = DefaultPaths, ...props }) => (
  <Switch path={path} pairs={pairs()} />
)

export const codeMenuContentPair = [Code, [Menu, Content]]
