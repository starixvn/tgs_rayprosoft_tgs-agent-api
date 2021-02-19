import React, { useRef, useEffect, Children, isValidElement } from "react"
import { css } from "@emotion/react"
import {
  Switch,
  Route,
  Link,
  useRouteMatch,
  useLocation
} from "react-router-dom"
import { useMenuCodes } from "./user/context"
import { SideNav } from "bumbag"
import memoize from "fast-memoize"
import { codeMenuContentPair as platformPair } from "./platform-management"
import {
  Code as PlatformCode,
  nameMessage as PlatformNameMessages,
  codeLabelPair as platformLabelPair
} from "./platform-management/menu"
import { codeMenuContentPair as playerManagementPair } from "./player-management"
import {
  Code as PlayerManagementCode,
  nameMessages as PlayerManagementNameMessages,
  codeLabelPair as playerManagementLabelPair
} from "./player-management/menu"
import { codeMenuContentPair as gameManagementPair } from "./game"
import {
  Code as GameManagementCode,
  nameMessages as GameManagementNameMessages,
  codeLabelPair as gameManagementLabelPair
} from "./game/menu"
import { codeMenuContentPair as reportManagementPair } from "./report"
import {
  Code as ReportManagementCode,
  nameMessages as ReportManagementNameMessages,
  codeLabelPair as reportManagementLabelPair
} from "./report/menu"
import { codeMenuContentPair as merchantPair } from "./merchant"
import {
  Code as MerchantCode,
  nameMessages as MerchantName,
  codeLabelPair as merchantLabelPair
} from "./merchant/menu"
import { codeMenuContentPair as systemPair } from "./system"
import {
  Code as SystemCode,
  nameMessages as SystemNameMessages,
  codeLabelPair as systemLabelPair
} from "./system/menu"
import { useLocale, getTrans } from "./i18n"

export const Menu = ({ title, url, codes, codePaths, itemLabel }) => {
  let locale = useLocale()
  let authCodes = useMenuCodes(codePaths)
  codes = codes ?? authCodes
  let items = codes
    .map((code) => {
      let label = itemLabel({ code, locale })

      return label ? (
        <MenuItem
          key={code}
          code={code}
          path={`${url}/${code}`}
          label={label}
        />
      ) : undefined
    })
    .filter((item) => !!item)

  return items && items.length ? (
    <SideNav.Level title={title}>
      <SideNav.Level>{items}</SideNav.Level>
    </SideNav.Level>
  ) : null
}

const MenuItem = ({ code, path, label, ...props }) => {
  let ref = useRef()
  let { pathname } = useLocation()
  let isActive = path == pathname

  useEffect(() => {
    isActive ? ref?.current?.focus() : ref?.current?.blur()
  }, [path, isActive])

  if (!label) {
    return null
  }

  return (
    <SideNav.Item navId={path} isActive={isActive}>
      <Link ref={ref} to={path}>
        {label}
      </Link>
    </SideNav.Item>
  )
}

export const allMenuContentPairs = () => [
  platformPair,
  playerManagementPair,
  gameManagementPair,
  reportManagementPair,
  merchantPair,
  systemPair
]

export const AllMenus = ({ codes }) => {
  let authCodes = useMenuCodes()
  codes = codes ?? authCodes
  let list = allMenuContentPairs()
  return codes
    .map((code) => <MenuByCode key={code} code={code} pairs={list} />)
    .filter((item) => !!item)
}

export const useLabel = ({ codes = [] }) => {
  let locale = useLocale()
  let label = moduleLabel
  let subLabelPairs = [
    playerManagementLabelPair,
    systemLabelPair,
    gameManagementLabelPair,
    reportManagementLabelPair,
    merchantLabelPair,
    platformLabelPair
  ]
  return findlLabel({
    locale,
    codes,
    label,
    subLabelPairs
  })
}

//TODO: cache?
export const findlLabel = ({ locale, codes = [], label, subLabelPairs }) => {
  let [code = "", ...rest] = codes
  code = code.toLowerCase()

  if (rest.length === 0) {
    return label({ locale, code })
  }

  let [, subLabel] = subLabelPairs.find(([id]) => id === code) ?? []

  return subLabel && subLabel({ locale, code, codes: rest })
}

const moduleLabel = ({ code = "", locale }) => {
  switch (code) {
    case PlatformCode:
      return getTrans(PlatformNameMessages, locale)
    case PlayerManagementCode:
      return getTrans(PlayerManagementNameMessages, locale)
    case GameManagementCode:
      return getTrans(GameManagementNameMessages, locale)
    case ReportManagementCode:
      return getTrans(ReportManagementNameMessages, locale)
    case MerchantCode:
      return getTrans(MerchantName, locale)
    case SystemCode:
      return getTrans(SystemNameMessages, locale)
    default:
      return ""
  }
}

const MenuByCode = ({ code = "", pairs = [] }) => {
  let [, pair] = pairs.find(([id]) => id === code) ?? []
  if (pair) {
    let [Menu] = pair
    return <Menu />
  }

  return null
}

export const find = (path = [], list = []) => {
  let [code, ...restPath] = path
  code = code.toLowerCase()
  let menu = list.find((item) => item?.code?.toLowerCase() === code) ?? {}

  if (restPath.length === 0) {
    return menu
  }

  return find(restPath, menu?.subMenus)
}

export const memoizedFind = memoize(find)
