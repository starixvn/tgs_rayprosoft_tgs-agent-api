import React from "react"
import { css } from "@emotion/react"
import { Spinner } from "bumbag"
import { getTrans, Trans, useLocale, useTrans } from "../../i18n"
import { Switch } from "../../switch/Switch"
import { Select } from "../../select/Select"

const Enabled = "Enabled"
const EnabledTrans = {
    zh: "开启",
    en: "Enabled"
}
const Disabled = "Disabled"
const DisabledTrans = {
    zh: "关闭",
    en: "Disabled"
}

export const PromotionStatusSelect = ({ placeholder, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "状态",
        en: "Status"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let options = [
        {
            label: useTrans(EnabledTrans),
            value: Enabled
        },
        {
            label: useTrans(DisabledTrans),
            value: Disabled
        }
    ]

    return <Select placeholder={placeholder} options={options} {...props} />
}

//TODO: remove disabled when bo-api ready
export const PromotionStatusSwitch = ({ value, disabled = true, ...props }) => {
    let checked = value === Enabled
    return (
        <Switch
            checked={checked}
            {...props}
            css={css`
                position: relative;
            `}
        />
    )
}
