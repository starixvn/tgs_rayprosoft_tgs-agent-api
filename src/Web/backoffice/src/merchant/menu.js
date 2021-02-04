import React, { useRef, useEffect, Children, isValidElement } from "react"
import { css } from "@emotion/react"
import { Menu as _Menu, findlLabel } from "../menu"
import {
  Code as ResellerCode,
  codeLabelPair as resellerPair
} from "./resellers/menu"
import {
  Code as LicenseeCode,
  codeLabelPair as licenseePair
} from "./licensees/menu"
import { Code as BrandCode, codeLabelPair as brandPair } from "./brands/menu"
import { getTrans, useTrans } from "../i18n"

export const nameMessages = {
  zh: "商户管理",
  en: "Merchant Management"
}

export const Name = () => useTrans(nameMessages)

export const Code = "merchant"

export const DefaultUrl = `/${Code}`

export const DefaultPaths = [Code]

export const Menu = ({
  title = <Name />,
  url = DefaultUrl,
  codes,
  paths = DefaultPaths,
  ...props
}) => (
  <_Menu
    title={title}
    url={url}
    codes={codes}
    codePaths={paths}
    itemLabel={label}
  />
)

const label = ({ locale, code }) => {
  switch (code) {
    case ResellerCode:
      return getTrans(
        {
          zh: "总公司列表",
          en: "Resellers"
        },
        locale
      )
    case LicenseeCode:
      return getTrans(
        {
          zh: "公司列表",
          en: "Licensees"
        },
        locale
      )
    case BrandCode:
      return getTrans(
        {
          zh: "品牌列表",
          en: "Brands"
        },
        locale
      )
    default:
      return ""
  }
}

export const codeLabelPair = [
  Code,
  ({ locale, codes }) =>
    findlLabel({
      locale,
      codes,
      label: label,
      subLabelPairs: [resellerPair, licenseePair, brandPair]
    })
]
