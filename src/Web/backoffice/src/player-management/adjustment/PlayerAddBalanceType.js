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

export const Deposit = "Deposit"
const DepositTrans = {
    zh: "充值",
    en: "Deposit"
}

export const Other = "Other"
const OtherTrans = {
    zh: "其他",
    en: "Other"
}

export const PlayerAddBalanceTypeSelect = ({
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
            label: useTrans(DepositTrans),
            value: Deposit
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

export default PlayerAddBalanceTypeSelect
