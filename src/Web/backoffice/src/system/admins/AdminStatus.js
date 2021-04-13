import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Trans, useTrans } from "../../i18n"
import Select from "../../select/Select"

const Disabled = "Disabled"
const DisabledTrans = {
    zh: "封锁",
    en: Disabled
}

const Enabled = "Enabled"
const EnabledTrans = {
    zh: "活跃",
    en: "Enabled"
}

export const AdminStatusLabel = ({ value }) => {
    switch (value) {
        case Disabled:
            return <Trans {...DisabledTrans} />

        case Enabled:
            return <Trans {...EnabledTrans} />

        default:
            return value
    }
}

export const AdminStatusSelect = ({ placeholder, ...props }) => {
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
