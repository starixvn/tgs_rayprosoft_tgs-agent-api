import memoize from "fast-memoize"
import { Code as SystemCode } from "./system"
import { label as systemLabel } from "./system/menu"

export const find = (path = [], list = []) => {
  let [code, ...restPath] = path
  code = code.toLowerCase()
  let menu = list.find((item) => item?.Code?.toLowerCase() === code) ?? {}

  if (restPath.length === 0) {
    return menu
  }

  return find(restPath, menu?.SubMenus)
  /*
  let items = path.reduce((list, code, index, list) => {
    code = code.toLowerCase()
    list =
      list.find((item) => item?.code?.toLowerCase() === code)?.subMenus ?? []

    return list
  }, list)

  let result = items.find((item) => item.code.toLowerCase()) ?? {}
  return result
  */
}

const isLast = (index, list = []) => list.length - 1 === index

export const findMemoized = memoize(find)

export const label = (codes = []) => {
  let [code, ...subMenuCodes] = codes
  code = code.toLowerCase()
  if (subMenuCodes.length === 0) {
    return toLabel(code)
  }

  let [, fn] =
    list().find(([key]) => {
      return key === code
    }) ?? []

  let result = (fn && fn(subMenuCodes)) || ""
  return result
}

//TODO: i18n
const toLabel = (code = "") => {
  switch (code) {
    case SystemCode:
      return "系统管理"
    default:
      return ""
  }
}

const list = () => [[SystemCode, systemLabel]]
