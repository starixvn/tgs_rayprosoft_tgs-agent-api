import React, { useRef, useEffect } from "react"
import { css } from "@emotion/core"
import {
  Code as PlayerListCode,
  Content as PlayerListContent
} from "./player-list"
import { Content as _MenuContent } from "../menu"
import { Code, DefaultCodePath, Menu } from "./menu"

export { Code } from "./menu"

export const codeMenuPair = () => [Code, Menu]

const codeContentPairs = () => [[PlayerListCode, PlayerListContent]]

export const Content = ({ codes, path = DefaultCodePath, ...props }) => (
  <_MenuContent path={path} pairs={codeContentPairs()} />
)

export const codeContentPair = () => [Code, Content]
