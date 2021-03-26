import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Text } from "bumbag"
import { useTrans, Trans } from "../i18n"
import Select from "../select/Select"

const Enabled = "Enabled"
const EnabledTrans = {
    zh: "正常",
    en: "Enabled"
}

const Disabled = "Disabled"
const DisabledTrans = {
    zh: "封锁",
    en: "Disabled"
}

const InBlacklist = "InBlacklist"
const InBlacklistTrans = {
    zh: "黑名单",
    en: "In Blacklist"
}

export const AgentStatusSelect = ({
    placeholder,
    placeholderDisabled,
    ...props
}) => {
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
        },
        {
            label: useTrans(InBlacklistTrans),
            value: InBlacklist
        }
    ]

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

export const AgentStatusLabel = ({ value }) => {
    let code = value
    switch (code) {
        case Enabled:
            return (
                <Text>
                    <Trans {...EnabledTrans} />
                </Text>
            )
        case Disabled:
            return (
                <Text>
                    <Trans {...DisabledTrans} />
                </Text>
            )
        case InBlacklist:
            return (
                <Text>
                    <Trans {...InBlacklistTrans} />
                </Text>
            )
    }
    return null
}
