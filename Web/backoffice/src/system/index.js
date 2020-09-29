import React, { useRef, useEffect } from "react"
import { css } from "@emotion/core"
import { Code as AdminCode, Content as AdminContent } from "./admins"
import { Code as GroupCode, Content as GroupContent } from "./groups"
import {
  Code as DepartmentCode,
  Content as DepartmentContent
} from "./departments"
import { Content as _MenuContent } from "../menu"
import { Code, DefaultCodePath, Menu } from "./menu"

export { Code } from "./menu"

export const codeMenuPair = () => [Code, Menu]

const codeContentPairs = () => [
  [AdminCode, AdminContent],
  [GroupCode, GroupContent],
  [DepartmentCode, DepartmentContent]
]

export const Content = ({ codes, path = DefaultCodePath, ...props }) => (
  <_MenuContent path={path} pairs={codeContentPairs()} />
)

export const codeContentPair = () => [Code, Content]
