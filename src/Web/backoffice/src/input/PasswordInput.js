import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { InputField } from "./Input"
import { useFormContext } from "react-hook-form"
import { useTrans } from "../i18n"
import SuperExpressive from "super-expressive"

const PasswordMinLength = 8
const PasswordMaxLength = 16

const OneOrMoreDigit = SuperExpressive().oneOrMore.digit.toRegex()

const OneOrMoreAlpha = SuperExpressive()
    .oneOrMore.anyOf.range("a", "z")
    .range("A", "Z")
    .end()
    .toRegex()

export const PasswordInputField = ({
    name = "",
    required = true,
    matchPassword = "",
    ...props
}) => {
    let requiredMessage = useTrans({
        zh: "必填栏",
        en: "Required field"
    })

    let minLengthMessage = useTrans({
        zh: `至少 ${PasswordMinLength} 个字`,
        en: `Up to  characters`
    })

    let maxLengthMessage = useTrans({
        zh: `最多${PasswordMaxLength}个字`,
        en: `Up to ${PasswordMaxLength} characters`
    })

    let atLeastOneAlphabetMessage = useTrans({
        zh: "至少一个字母",
        en: "At least one alphabet"
    })

    let atLeastOneNumberMessage = useTrans({
        zh: "至少一个数字",
        en: "At least one number"
    })

    let passwordNotMatchMessage = useTrans({
        zh: "密码不符",
        en: "Password not match"
    })

    return (
        <InputField
            type="password"
            name={name}
            required={required}
            minLength={PasswordMinLength}
            maxLength={PasswordMaxLength}
            validate={{
                atLeastOneAlphabet: (value) => {
                    return (
                        OneOrMoreAlpha.test(value) || atLeastOneAlphabetMessage
                    )
                },
                atLeastOneNumber: (value) =>
                    OneOrMoreDigit.test(value) || atLeastOneNumberMessage,
                matchPassword: (value) => {
                    if (matchPassword) {
                        return (
                            matchPassword === value || passwordNotMatchMessage
                        )
                    }

                    return true
                }
            }}
            {...props}
        />
    )
}
