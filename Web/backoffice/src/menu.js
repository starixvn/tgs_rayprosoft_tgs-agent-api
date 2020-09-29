import React, { useRef, useEffect } from "react"
import { css } from "@emotion/core"
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
import { codeMenuContentPair as platformPair } from "./platform"
import { codeMenuContentPair as playerManagementPair } from "./player-management"
import { codeMenuContentPair as systemPair } from "./system"

export const Menu = ({ title, url, codes, codePaths, itemLabel }) => {
  let authCodes = useMenuCodes(codePaths)
  codes = codes ?? authCodes

  return (
    <SideNav.Level title={title}>
      <SideNav.Level>
        {codes.map((code) => (
          <MenuItem
            key={code}
            code={code}
            path={`${url}/${code}`}
            itemLabel={itemLabel}
          />
        ))}
      </SideNav.Level>
    </SideNav.Level>
  )
}

const MenuItem = ({ code, path, itemLabel, ...props }) => {
  let ref = useRef()
  let { pathname } = useLocation()
  let isActive = path == pathname

  useEffect(() => {
    isActive ? ref?.current.focus() : ref?.current.blur()
  }, [path, isActive])

  return (
    <SideNav.Item navId={path} isActive={isActive}>
      <Link ref={ref} to={path}>
        {itemLabel(code)}
      </Link>
    </SideNav.Item>
  )
}

const pairs = () => [
  // platformPair,
  playerManagementPair,
  systemPair
]

export const AllMenus = ({ codes }) => {
  let authCodes = useMenuCodes()
  codes = codes ?? authCodes
  let list = pairs()
  return codes.map((code) => <MenuByCode key={code} code={code} pairs={list} />)
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

export const findMenuLabel = (toLabel, subMenuList) => (codes = []) => {
  let [code, ...subMenuCodes] = codes
  code = code.toLowerCase()
  if (subMenuCodes.length === 0) {
    return toLabel(code)
  }

  let subMenuCodeLabelList = subMenuList()
  let [, subMenuLabel] = subMenuCodeLabelList.find(([id]) => id === code) ?? []
  return (subMenuLabel && subMenuLabel(subMenuCodes)) ?? ""
}
