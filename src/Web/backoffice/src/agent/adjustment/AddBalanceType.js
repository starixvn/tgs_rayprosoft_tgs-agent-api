import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
    Dialog,
    useModalState,
    Stack,
    Heading,
    Text,
    Box,
    Set,
    FieldStack
} from "bumbag"
import { useTrans } from "../../i18n"
import Select from "../../select/Select"

export const Bonus = "Bonus"
const BonusTrans = {
    zh: "红利",
    en: "Bonus"
}

export const Recharge = "Recharge"
const RechargeTrans = {
    zh: "充值",
    en: "Recharge"
}

export const Dividend = "Dividend"
const DividendTrans = {
    zh: "分紅",
    en: "Dividend"
}

export const Other = "Other"
const OtherTrans = {
    zh: "其他",
    en: "Other"
}

export const AddBalanceTypeSelect = ({
    placeholder,
    placeholderDisabled = false,
    ...props
}) => {
    let options = [
        {
            label: useTrans(BonusTrans),
            value: Bonus
        },
        {
            label: useTrans(RechargeTrans),
            value: Recharge
        },
        {
            label: useTrans(DividendTrans),
            value: Dividend
        },
        {
            label: useTrans(OtherTrans),
            value: Other
        }
    ]

    let defaultPlaceholder = useTrans({
        zh: "交易类型",
        en: "Transaction Type"
    })

    placeholder = placeholder ?? defaultPlaceholder
    if (placeholder) {
        options = [
            {
                label: placeholder,
                value: "",
                disabled: placeholderDisabled
            },
            ...options
        ]
    }

    return <Select options={options} {...props} />
}

export default AddBalanceTypeSelect
