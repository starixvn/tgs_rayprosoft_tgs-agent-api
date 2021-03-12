import React, { useRef, useEffect } from "react"
import { css } from "@emotion/react"
import { Code, Menu, DefaultPaths } from "./menu"
import { Switch } from "../content"
import {
    Code as AnnouncementCode,
    Content as AnnouncementContent
} from "./announcement"
import {
    Code as StationLetterCode,
    Content as StationLetterContent
} from "./station-letter"
import { Code as VipLevelCode, Content as VipLevelContent } from "./viplevel"

export { Code }

const pairs = () => [
    [AnnouncementCode, AnnouncementContent],
    [StationLetterCode, StationLetterContent],
    [VipLevelCode, VipLevelContent]
]

export const Content = ({ codes, path = DefaultPaths, ...props }) => (
    <Switch path={path} pairs={pairs()} />
)

export const codeMenuContentPair = [Code, [Menu, Content]]
