import { Code as AdminCode } from "./admins"
import { label as adminLabel } from "./admins/menu"
import { Code as GroupCode } from "./groups"
import { label as groupLabel } from "./groups/menu"
import { Code as DepartmentCode } from "./departments"
import { label as departmentLabel } from "./departments/menu"

export const label = (codes = []) => {
  let [code, ...subMenuCodes] = codes
  code = code.toLowerCase()
  if (subMenuCodes.length === 0) {
    return toLabel(code)
  }

  code = code.toLowerCase()
  let [, fn] = list().find(([key]) => key === code) ?? []

  return (fn && fn(subMenuCodes)) || ""
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

const list = () => [
  [AdminCode, adminLabel],
  [GroupCode, groupLabel],
  [DepartmentCode, departmentLabel]
]
