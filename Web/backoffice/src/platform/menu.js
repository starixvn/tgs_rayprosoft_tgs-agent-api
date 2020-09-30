import React, { useRef, useEffect } from "react"
import { css } from "@emotion/core"
import { Menu as _Menu, findMenuLabel } from "../menu"
import {
  Code as AnnouncementCode,
  label as announcementLabel
} from "./announcement/menu"
import {
  Code as StationLetterCode,
  label as stationLetterLabel
} from "./station-letter/menu"

//TODO: i18n
export const name = () => "平台管理"

export const Code = "platform"

export const DefaultUrl = `/${Code}`

export const DefaultPaths = [Code]

export const Menu = ({
  title = name(),
  url = DefaultUrl,
  codes,
  paths = DefaultPaths,
  ...props
}) => (
  <_Menu
    title={title}
    url={url}
    codes={codes}
    codePaths={paths}
    itemLabel={toLabel}
  />
)

const subMenuList = () => [
  [AnnouncementCode, announcementLabel],
  [StationLetterCode, stationLetterLabel]
]

export const label = (codes = []) => {
  let findLabelBy = findMenuLabel(toLabel, subMenuList)
  return findLabelBy(codes)
}

//TODO: i18n
export const toLabel = (code = "") => {
  switch (code) {
    case AnnouncementCode:
      return "公告管理"
    case StationLetterCode:
      return "站内信"
    default:
      return ""
  }
}
