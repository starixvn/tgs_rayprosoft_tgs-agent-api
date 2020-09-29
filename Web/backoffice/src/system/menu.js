import React, { useRef, useEffect } from "react"
import { css } from "@emotion/core"
import { Menu as _Menu, findMenuLabel } from "../menu"
import { Code as AdminCode } from "./admins"
import { label as adminLabel } from "./admins/menu"
import { Code as GroupCode } from "./groups"
import { label as groupLabel } from "./groups/menu"
import { Code as DepartmentCode } from "./departments"
import { label as departmentLabel } from "./departments/menu"

export const Code = "system"

export const DefaultUrl = `/${Code}`

export const DefaultCodePath = [Code]

//TODO: i18n
const name = () => "系统设置"

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

export const label = (codes = []) => {
  let findLabelBy = findMenuLabel(toLabel, subMenuList)
  return findLabelBy(codes)
}

//TODO: i18n
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
  [GroupCode, groupLabel],
  [DepartmentCode, departmentLabel]
]
