import React, { useRef, useEffect } from "react"
import { css } from "@emotion/core"
import { Menu as _Menu, findMenuLabel, findlLabel } from "../menu"
import {
  Code as PlayerListCode,
  label as playerListLabel,
  codeLabelPair as playerListPair,
} from "./player-list/menu"
import { getTrans, useTrans } from "../i18n"

export const Name = ({}) => useTrans(nameMessages)

export const nameMessages = {
  zh: "玩家管理",
  en: "Player management"
}

export const Code = "playermanagement"

export const DefaultUrl = `/${Code}`

export const DefaultCodePath = [Code]

//TODO: itemLabel wit locale
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
    itemLabel={toLabel}
  />
)

const labelOf = ({ locale, code }) => {
  switch (code) {
    case PlayerListCode:
      return getTrans({
        zh: "玩家列表",
        en: "Players"
      }, locale)
    default:
      return ""
  }
}

export const codeLabelPair = [
  Code, 
  ({ locale, codes }) => findlLabel({
    locale,
    codes,
    label: labelOf, 
    subLabelPairs: [ playerListPair ]
  })
]

const subMenuList = () => [[PlayerListCode, playerListLabel]]

//TODO: rm
export const label = (codes = []) => {
  let findLabelBy = findMenuLabel(toLabel, subMenuList)
  return findLabelBy(codes)
}

//TODO: rm
export const toLabel = (code = "") => {
  switch (code) {
    case PlayerListCode:
      return "玩家列表"
    default:
      return ""
  }
}
