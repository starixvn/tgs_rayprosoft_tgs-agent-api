import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Box, Set, FieldStack } from "bumbag"
import { Trans, useTrans } from "../i18n"
import { NormalLabel } from "../form/Label"
import { useFormContext, useFieldArray } from "react-hook-form"
import { LanguageCheckboxGroupTable } from "../language/LanguageCheckboxGroup"

export const LanguageFieldsBox = ({ readonly }) => {
    let { watch, control } = useFormContext()

    let fieldsName = "languages"
    let fieldArrayContext = useFieldArray({
        control,
        name: fieldsName,
        keyName: "_id"
    })
    let languages = watch(fieldsName)

    if (!languages || languages?.length == 0) {
        return null
    }

    return (
        <>
            <NormalLabel>
                <Trans zh="语言" en="Language" />
            </NormalLabel>
            <LanguageCheckboxGroupTable
                fieldArrayContext={fieldArrayContext}
                disabled={readonly}
            />
        </>
    )
}

export const toCheckedCultureCodes = (languages = []) => {
    return languages
        .filter(({ checked }) => checked)
        .map(({ cultureCode }) => cultureCode)
}

export default LanguageFieldsBox
