import React, { Fragment, useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useTrans } from "../i18n"
import { Spinner, Button, Group } from "bumbag"
import { NumberInput } from "../input/NumberInput"

export const NumberRangeInputGroup = ({
    names = [],
    type = "number",
    required,
    min,
    max,
    orientation = "horizontal",
    disabled,
    size = "small",
    inputFieldProps = {},
    separator: Separator = Button,
    ...props
}) => {
    return (
        <Group orientation={orientation} {...props}>
            {names.map((name, index) => {
                let hasNext = index + 1 < names.length

                let isDisabledList = Array.isArray(disabled)
                let isDisabled = isDisabledList ? disabled[index] : disabled

                let isRequiredList = Array.isArray(required)
                let isRequired =
                    !isDisabled && (isRequiredList ? required[index] : required)

                let isMinList = Array.isArray(min)
                let isMaxList = Array.isArray(max)

                return (
                    <NumberInput
                        key={name}
                        name={name}
                        type={type}
                        disabled={isDisabled}
                        required={isRequired}
                        min={isMinList ? min[index] : min}
                        max={isMaxList ? max[index] : max}
                        size={size}
                        flex={hasNext ? "1.1" : "1"}
                        addonAfter={
                            hasNext && (
                                <Separator
                                    isStatic
                                    size={size}
                                    borderRadius="0"
                                    tabIndex="-1"
                                >
                                    -
                                </Separator>
                            )
                        }
                        {...inputFieldProps}
                    />
                )
            })}
        </Group>
    )
}

export default NumberRangeInputGroup
