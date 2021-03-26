import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Text } from "bumbag"
import { useTrans, Trans } from "../i18n"
import Select from "../select/Select"

const Done = "DoneForConfigurePercentage"
const DoneTrans = {
    zh: "正常",
    en: "Normal"
}

const WaitForConfigurePercentage = "WaitForConfigurePercentage"
const WaitForConfigurePercentageTrans = {
    zh: "需要配置比例",
    en: "Config Required"
}

export const isAgentConfigDone = (value) => value === Done

export const isAgentConfigRequired = (value) =>
    value === WaitForConfigurePercentage

export const AgentConfigStatusSelect = ({
    placeholder,
    placeholderDisabled,
    ...props
}) => {
    let options = [
        {
            label: useTrans(DoneTrans),
            value: Done
        },
        {
            label: useTrans(WaitForConfigurePercentageTrans),
            value: WaitForConfigurePercentage
        }
    ]

    let defaultPlaceholder = useTrans({
        zh: "配置状态",
        en: "Config Status"
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

export const AgentConfigStatusLabel = ({ value = "", ...props }) => {
    switch (value) {
        case Done:
            return (
                <Text {...props}>
                    <Trans {...DoneTrans} />
                </Text>
            )
        case WaitForConfigurePercentage:
            return (
                <Text {...props}>
                    <Trans {...WaitForConfigurePercentageTrans} />
                </Text>
            )
        default:
            return value
    }
}
