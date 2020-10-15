import React, { useRef, useEffect } from "react"
import { css } from "@emotion/core"
import { Menu as _Menu, findlLabel } from "../menu"
import { Code as AdminCode, codeLabelPair as adminPair, label as adminLabel } from "./admins/menu"
import { Code as GroupCode, codeLabelPair as rolePair, label as roleLabel } from "./groups/menu"
import { Code as DepartmentCode, codeLabelPair as departmentPair, label as departmentLabel } from "./departments/menu"
import { getTrans, useTrans } from "../i18n"

export const Code = "system"

export const DefaultUrl = `/${Code}`

export const DefaultCodePath = [Code]

export const Name = ({}) => useTrans(nameMessages)

export const nameMessages = {
  zh: "系统设置",
  en: "System Config"
}

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
    case AdminCode:
      return getTrans({
        zh: "管理员列表",
        en: "Admins"
      }, locale)
    case GroupCode:
      return getTrans({
        zh: "权限组管理",
        en: "Roles"
      }, locale)
    case DepartmentCode:
      return getTrans(
        {
          zh: "部门管理",
          en: "Departments"
        }, locale)
    default:
      return ""
  }
}

export const codeLabelPair = [ Code, 
  ({ locale, codes }) => findlLabel({ 
    locale, 
    codes, 
    label: labelOf , 
    subLabelPairs: [ adminPair, rolePair, departmentPair ]
  })
]

//TODO: rm ?
export const label = (codes = []) => {
  let findLabelBy = findMenuLabel(toLabel, subMenuList)
  return findLabelBy(codes)
}

//TODO: rm?
export const toLabel = (code = "") => {
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

const subMenuList = () => [
  [AdminCode, adminLabel],
  [GroupCode, roleLabel],
  [DepartmentCode, departmentLabel]
]