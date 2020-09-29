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
import { Route as AuthRouter } from "./user/auth"
import { SideNav } from "bumbag"
import PageNotFound from "./page/NotFound"
import memoize from "fast-memoize"
import { Code as SystemCode } from "./system"
import { label as systemLabel } from "./system/menu"
import {
  codeMenuPair as playerManagementMenuPair,
  codeContentPair as playerManagementContentPair
} from "./player-management"
import {
  codeMenuPair as systemMenuPair,
  codeContentPair as systemContentPair
} from "./system"
import { PageContent } from "bumbag"

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

export const Content = ({ codes, path, pairs = [], ...props }) => {
  let route = useRouteMatch()
  let authCodes = useMenuCodes(path)
  codes = codes ?? authCodes

  return (
    <Switch>
      {codes.map((code) => {
        return (
          <Route key={code} path={`${route?.path}/${code}`}>
            <FindValueByCode code={code} pairs={pairs} />
          </Route>
        )
      })}
      <Route>
        <PageNotFound />
      </Route>
    </Switch>
  )
}

const codeMenuPairs = () => [playerManagementMenuPair(), systemMenuPair()]

export const addCodeMenuPair = (entry = []) => codeMenuPairs.push(entry)

export const Menus = ({ codes }) => {
  let authCodes = useMenuCodes()
  codes = codes ?? authCodes
  let pairs = codeMenuPairs()
  return codes.map((code) => (
    <FindValueByCode key={code} code={code} pairs={pairs} />
  ))
}

const codeContentPairs = () => [
  playerManagementContentPair(),
  systemContentPair()
]

export const addCodeContentPair = (entry = []) => codeContentPairs.push(entry)

export const Contents = ({ codes }) => {
  let authCodes = useMenuCodes()
  codes = codes ?? authCodes
  let pairs = codeContentPairs()
  return (
    <Switch>
      {codes.map((code) => {
        let path = `/${code}`

        return (
          <AuthRouter key={code} path={path}>
            <FindValueByCode code={code} pairs={pairs} />
          </AuthRouter>
        )
      })}

      <AuthRouter path="*">
        <PageContent></PageContent>
      </AuthRouter>
    </Switch>
  )
}

const FindValueByCode = ({ code = "", pairs = [] }) => {
  let [, Value] = pairs.find(([id]) => id === code) ?? []
  if (Value) {
    return <Value />
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

export const label = findMenuLabel(toLabel, subMenuList)

//TODO: i18n
export const toLabel = (code = "") => {
  switch (code) {
    case SystemCode:
      return "系统管理"
    default:
      return ""
  }
}

const subMenuList = () => [[SystemCode, systemLabel]]
