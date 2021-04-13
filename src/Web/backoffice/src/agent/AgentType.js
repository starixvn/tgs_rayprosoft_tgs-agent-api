import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Text } from "bumbag"
import { useTrans, Trans } from "../i18n"
import Select from "../select/Select"

const Cash = "Cash"
const CashTrans = {
    zh: "现金",
    en: "Cash"
}

const Credit = "Credit"
const CreditTrans = {
    zh: "信用",
    en: "Credit"
}

export const AgentTypeSelect = ({ placeholder, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "代理类型",
        en: "Agent Type"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let options = [
        {
            label: useTrans(CashTrans),
            value: Cash
        },
        {
            label: useTrans(CreditTrans),
            value: Credit
        }
    ]

    return <Select placeholder={placeholder} options={options} {...props} />
}

export const AgentTypeLabel = ({ value }) => {
    let code = value
    switch (code) {
        case Cash:
            return (
                <Text>
                    <Trans {...CashTrans} />
                </Text>
            )
        case Credit:
            return (
                <Text>
                    <Trans {...CreditTrans} />
                </Text>
            )
        default:
            return value
    }
}
