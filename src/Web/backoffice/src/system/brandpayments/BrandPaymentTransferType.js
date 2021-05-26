import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Trans, useTrans } from "../../i18n"
import Select from "../../select/Select"

export const Deposit = "Deposit"
const DepositTrans = {
    zh: "Deposit",
    en: "Deposit"
}

export const Withdrawal = "Withdrawal"
const WithdrawalTrans = {
    zh: "Withdrawal",
    en: "Withdrawal"
}

export default {
    Deposit,
    Withdrawal
}

export const BrandPaymentTransferTypeSelect = ({ placeholder, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "Transfer Type",
        en: "Transfer Type"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let options = [
        {
            label: useTrans(DepositTrans),
            value: Deposit
        },
        {
            label: useTrans(WithdrawalTrans),
            value: Withdrawal
        }
    ]

    return <Select placeholder={placeholder} options={options} {...props} />
}
