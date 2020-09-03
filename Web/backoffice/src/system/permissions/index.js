import React, { useEffect, useRef } from "react"
import { css } from "@emotion/core"
import {
  Switch,
  Route,
  Link,
  Redirect,
  useHistory,
  useLocation,
  useRouteMatch
} from "react-router-dom"
import { SideNav } from "bumbag"
import { useAuthCodes } from "../../user/context"
import { Code as AdminCode, Content as AdminContent } from "./admins"
import { Code as GroupCode, Content as GroupContent } from "./groups"
import {
  Code as DepartmentCode,
  Content as DepartmentContent
} from "./departments"
import PageNotFound from "../../page/NotFound"

export const Code = "permissions"

export const DefaultUrl = `/${Code}`

//TODO: i18n
const name = () => "权限设置"

export const Menu = ({
  title = name(),
  url = "/",
  codes,
  codePaths = [],
  ...props
}) => {
  url = `${url}/${Code}`
  codePaths = [...codePaths, Code]

  let authCodes = useAuthCodes(codePaths)
  codes = codes ?? authCodes

  //TODO: rm
  codes = [AdminCode, GroupCode, DepartmentCode]
  console.log("TODO: rm system/permissions menu codes", [
    AdminCode,
    GroupCode,
    DepartmentCode
  ])

  return (
    <SideNav.Level title={title}>
      <SideNav.Level>
        {codes.map((code) => (
          <MenuItem key={code} code={code} url={url} />
        ))}
      </SideNav.Level>
    </SideNav.Level>
  )
}

const MenuItem = ({ code, url, ...props }) => {
  let ref = useRef()
  let path = `${url}/${code}`

  useEffect(() => {
    console.log("TODO: blur?")
    ref?.current.blur()
  })

  return (
    <SideNav.Item navId={path}>
      <Link ref={ref} to={path}>
        {label(code)}
      </Link>
    </SideNav.Item>
  )

  //TODO: seems cause loop on debug mode
  /*
  let ref = useRef()
  let { pathname } = useLocation()
  let path = `${url}/${code}`
  let isActive = path == pathname
  useEffect(() => {
    if (isActive) {
      ref?.current.focus()
    }
  }, [isActive])

  return (
    <SideNav.Item navId={path} isActive={isActive}>
      <Link ref={ref} to={path}>
        {label(code)}
      </Link>
    </SideNav.Item>
  )
  */
}

export const Content = ({ codes, codePaths }) => {
  let route = useRouteMatch()

  let authCodes = useAuthCodes(codePaths)
  codes = codes ?? authCodes

  //TODO: rm
  codes = [AdminCode, GroupCode, DepartmentCode]
  console.log("TODO: rm system/permissions/ content codes", [
    AdminCode,
    GroupCode,
    DepartmentCode
  ])

  return (
    <Switch>
      {codes.map((code) => (
        <Route key={code} path={path(route?.path, code)}>
          <Body code={code} />
        </Route>
      ))}
      <Route>
        <PageNotFound />
      </Route>
    </Switch>
  )
}

const Body = ({ code }) => {
  switch (code) {
    case AdminCode:
      return <AdminContent />
    case GroupCode:
      return <GroupContent />
    case DepartmentCode:
      return <DepartmentContent />
    default:
      return <PageNotFound />
  }
}

//TODO: i18n
const label = (code = "") => {
  switch (code) {
    case AdminCode:
      return "管理员列表"
    case GroupCode:
      return "权限组管理"
    case DepartmentCode:
      return "部门管理"
    default:
      return ""
  }
}

const path = (url = "/", path) => `${url}/${path}`
