import React, { useRef, useEffect } from "react"
import { css } from "@emotion/core"
import { Menu as _Menu, findMenuLabel } from "../menu"
import {
  Code as PlayerListCode,
  label as playerListLabel
} from "./player-list/menu"

//TODO: i18n
export const name = () => "玩家管理"

export const Code = "playermanagement"

export const DefaultUrl = `/${Code}`

export const DefaultCodePath = [Code]

export const Menu = ({
  title = name(),
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

const subMenuList = () => [[PlayerListCode, playerListLabel]]
export const label = (codes = []) => {
  let findLabelBy = findMenuLabel(toLabel, subMenuList)
  return findLabelBy(codes)
}

//TODO: i18n
export const toLabel = (code = "") => {
  switch (code) {
    case PlayerListCode:
      return "玩家列表"
    default:
      return ""
  }
}
