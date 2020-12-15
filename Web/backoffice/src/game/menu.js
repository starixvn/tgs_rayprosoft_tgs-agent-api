import React, { useRef, useEffect } from "react"
import { css } from "@emotion/react"
import { Menu as _Menu, findlLabel } from "../menu"
import {
  Code as GameProviderCode,
  codeLabelPair as gameProviderPair
} from "./providers/menu"
import { Code as GameCode, codeLabelPair as gamePair } from "./games/menu"
import { getTrans, useTrans } from "../i18n"

export const Code = "game"

export const DefaultUrl = `/${Code}`

export const DefaultCodePath = [Code]

export const Name = ({}) => useTrans(nameMessages)

export const nameMessages = {
  zh: "游戏管理",
  en: "Game Management"
}

export const Menu = ({
  title = <Name />,
  url = DefaultUrl,
  codes,
  codePaths = DefaultCodePath,
  ...props
}) => (
  <_Menu
    title={title}
    url={url}
    codes={codes}
    codePaths={codePaths}
    itemLabel={label}
  />
)

const label = ({ locale, code }) => {
  switch (code) {
    case GameProviderCode:
      return getTrans(
        {
          zh: "厂商列表",
          en: "Provider List"
        },
        locale
      )
    case GameCode:
      return getTrans(
        {
          zh: "游戏列表",
          en: "Game List"
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
      label,
      subLabelPairs: [gameProviderPair, gamePair]
    })
]
