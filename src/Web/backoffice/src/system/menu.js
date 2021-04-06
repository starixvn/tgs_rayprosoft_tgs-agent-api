import React, { useRef, useEffect } from "react"
import { css } from "@emotion/react"
import { Menu as _Menu, findlLabel } from "../menu"
import { Code as AdminCode, codeLabelPair as adminPair } from "./admins/menu"
import { Code as GroupCode, codeLabelPair as rolePair } from "./roles/menu"
import {
    Code as DepartmentCode,
    codeLabelPair as departmentPair
} from "./departments/menu"
import {
    Code as BrandPaymentCode,
    codeLabelPair as brandPaymentPair
} from "./brandpayments/menu"
import {
    Code as SettingsCode,
    codeLabelPair as settingsPair
} from "./setting/menu"
import { getTrans, useTrans } from "../i18n"

export const Code = "system"

export const DefaultUrl = `/${Code}`

export const DefaultCodePath = [Code]

export const Name = ({}) => useTrans(nameMessages)

export const nameMessages = {
    zh: "系统设置",
    en: "System Config"
}

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
        itemLabel={labelOf}
    />
)

const labelOf = ({ locale, code }) => {
    switch (code) {
        case AdminCode:
            return getTrans(
                {
                    zh: "管理员列表",
                    en: "Admins"
                },
                locale
            )
        case GroupCode:
            return getTrans(
                {
                    zh: "权限组管理",
                    en: "Roles"
                },
                locale
            )
        case DepartmentCode:
            return getTrans(
                {
                    zh: "部门管理",
                    en: "Departments"
                },
                locale
            )
        case BrandPaymentCode:
            return getTrans(
                {
                    zh: "金流管理",
                    en: "Payments"
                },
                locale
            )
        case SettingsCode:
            return getTrans(
                {
                    zh: "设定",
                    en: "Settings"
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
            label: labelOf,
            subLabelPairs: [
                adminPair,
                rolePair,
                departmentPair,
                brandPaymentPair,
                settingsPair
            ]
        })
]
