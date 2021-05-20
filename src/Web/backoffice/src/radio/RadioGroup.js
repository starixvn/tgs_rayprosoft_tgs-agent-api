import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext, Controller } from "react-hook-form"
import { RadioGroup as _RadioGroup } from "bumbag"

export const RadioGroup = ({ name, defaultValue, ...props }) => {
    let { control } = useFormContext() ?? {}

    return (
        <Controller
            control={control}
            name={name}
            defaultValue={defaultValue}
            shouldUnregister={true}
            render={({
                field: { ref, ...formProps },
                fieldState: { invalid, isTouched, isDirty }
            }) => {
                return <_RadioGroup {...formProps} {...props} />
            }}
        />
    )
}

export default RadioGroup
