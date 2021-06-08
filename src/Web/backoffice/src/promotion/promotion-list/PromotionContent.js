import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import {
    Box,
    Text,
    Button,
    Set,
    Icon,
    useModalState,
    Dialog,
    Card,
    Flex,
    Heading,
    FieldStack
} from "bumbag"
import { Update as UpdateMenu, View as ViewMenu } from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import {
    AddDisclosure,
    EditDisclosure,
    ViewDisclosure
} from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import { Trans, useTrans } from "../../i18n"
import {
    Controller,
    useForm,
    useFormContext,
    useWatch,
    useFieldArray
} from "react-hook-form"
import { InputField, Input } from "../../input/Input"
import clone from "clone-deep"
import { useAddPromotion, useUpdatePromotion } from "./client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel } from "../../form/Label"
import { DateInputRange } from "../../date/DateTimeRangeInput"
import { VipLevelCheckboxGroup } from "../../vip-levels/CheckboxGroup"
import AlertDialog from "../../dialog/Alert"
import Checkbox from "../../checkbox"
import {
    GameTypeGroupSelect,
    GameTypeSelect,
    useGameTypeGroupValueOf
} from "../../game/GameType"
import Separator from "../../layout/Separator"
import {
    PromotionTypeGroup,
    PromotionTypeGroupSelect,
    PromotionTypeSelect,
    usePromotionGroupValueOf
} from "../PromotionType"
import { toCheckedGameProviderIds } from "../../game/providers/GameProviderCheckboxGroup"
import { BrandGameProviderCheckboxGroupTable } from "../../game/brandproviders/BrandGameProviderCheckboxGroup"
import parseISO from "date-fns/parseISO"
import { today } from "../../date/range"
import { useBrandGameProviderList } from "../../game/brandproviders/client"
import { CloseButtonFooter } from "../../form/ConfirmButtonFooter"
import {
    NumberInput,
    NumberInputField,
    PercentageNumberInputField
} from "../../input/NumberInput"
import { Switch } from "../../switch/Switch"
import { HtmlEditor } from "../../text/editor/HtmlEditor"
import { Tabs, Tab, TabPanel, TabList, useTabState } from "../../tab"
import Backdrop from "../../dialog/Backdrop"
import { ConfirmText } from "../../button/ActionButtons"
import head from "../../list/head"

export const PromotionContentDisclosure = ({
    name = "content",
    readOnly = false,
    ...props
}) => {
    let { control, setValue } = useFormContext()
    let value = useWatch({
        control,
        name
    })

    let dialog = useModalState()
    let editable = !readOnly

    //TODO: required rules
    return (
        <Controller
            name={name}
            control={control}
            defaultValue={value}
            shouldUnregister={true}
            render={({
                field: { value, name, ...fieldProps },
                fieldState,
                formState
            }) => (
                <>
                    {readOnly && <ViewDisclosure {...dialog} />}
                    {editable && <EditDisclosure {...dialog} />}
                    {dialog.visible && (
                        <PromotionContentDialog
                            value={value}
                            onChange={(data) => {
                                setValue(name, data)
                            }}
                            dialog={dialog}
                        />
                    )}
                </>
            )}
        />
    )
}

const FieldName = "value"
const PromotionContentDialog = ({ value = {}, readOnly, onChange, dialog }) => {
    let editTitle = useTrans({
        zh: "编辑内容",
        en: "Edit Content"
    })
    let viewTitle = useTrans({
        zh: "查看内容",
        en: "View Content"
    })

    return (
        <Backdrop {...dialog}>
            <FormDialog
                defaultValues={clone(value)}
                onSubmit={(data) => {
                    console.log("onSubmit", data)
                    if (!readOnly) {
                        onChange?.(data)
                    }
                    dialog.hide()
                }}
                stopPropagation={true}
                dialog={dialog}
                hideBackdrop={true}
                title={readOnly ? viewTitle : editTitle}
                submitText={<ConfirmText />}
                width="80vw"
                maxWidth="80vw"
            >
                <FormBody readOnly={readOnly} />
            </FormDialog>
        </Backdrop>
    )
}

const FormBody = ({ readOnly }) => {
    return (
        <FieldValues>
            <NormalLabel>
                <Trans zh="活动标题" en="Title" />
            </NormalLabel>
            <Box>
                <Input name={`title`} readOnly={readOnly} />
            </Box>

            <NormalLabel>
                <Trans zh="活动子标题" en="Subtitle" />
            </NormalLabel>
            <Box>
                <Input name={`subtitle`} readOnly={readOnly} />
            </Box>

            <NormalLabel>
                <Trans zh="活动Banner" en="Banner" />
            </NormalLabel>
            <Box></Box>

            <NormalLabel>
                <Trans zh="活动内容" en="Content" />
            </NormalLabel>
            <Box>
                <HtmlEditor name={`content`} readOnly={readOnly} />
            </Box>

            <NormalLabel>
                <Trans zh="活动条款与条规" en="Terms" />
            </NormalLabel>
            <Box>
                <HtmlEditor name={`terms`} readOnly={readOnly} />
            </Box>
        </FieldValues>
    )
}
