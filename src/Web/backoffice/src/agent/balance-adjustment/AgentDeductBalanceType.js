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

export const Transfer = "Transfer"
const TransferTrans = {
    zh: "转账",
    en: "Transfer"
}

export const Other = "Other"
const OtherTrans = {
    zh: "其他",
    en: "Other"
}

export const AddBalanceTypeSelect = ({ placeholder, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "交易类型",
        en: "Transaction Type"
    })
    placeholder = placeholder ?? defaultPlaceholder

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
            label: useTrans(TransferTrans),
            value: Transfer
        },
        {
            label: useTrans(OtherTrans),
            value: Other
        }
    ]

    return <Select placeholder={placeholder} options={options} {...props} />
}

export default {
    Bonus,
    Recharge,
    Dividend,
    Transfer,
    Other
}
