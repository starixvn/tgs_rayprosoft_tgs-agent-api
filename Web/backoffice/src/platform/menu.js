import React, { useRef, useEffect } from "react"
import { css } from "@emotion/core"
import { Menu as _Menu, findlLabel } from "../menu"
import {
  Code as AnnouncementCode,
  codeLabelPair as announcementPair
} from "./announcement/menu"
import {
  Code as StationLetterCode,
  codeLabelPair as stationLetterPair
} from "./station-letter/menu"
import { getTrans, useTrans } from "../i18n"

export const Name = () => useTrans(nameMessage)

export const nameMessage = {
  zh: "平台管理",
  en: "Platform"
}

export const Code = "platform"

export const DefaultUrl = `/${Code}`

export const DefaultPaths = [Code]

export const Menu = ({
  title = <Name />,
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
    itemLabel={labelOf}
  />
)

const labelOf = ({ locale, code }) => {
  switch (code) {
    case AnnouncementCode:
      return getTrans(
        {
          zh: "公告管理",
          en: "Announcements"
        },
        locale
      )
    case StationLetterCode:
      return getTrans(
        {
          zh: "站内信",
          en: "Station Letters"
        },
        locale
      )
    default:
      return ""
  }
}

export const codeLabelPair = [
  Code,
  ({ locale, codes }) =>
    findlLabel({
      locale,
      codes,
      label: labelOf,
      subLabelPairs: [announcementPair, stationLetterPair]
    })
]
