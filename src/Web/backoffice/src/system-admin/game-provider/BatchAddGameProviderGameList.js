import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import {
    Stack,
    Heading,
    Box,
    Button,
    Text,
    Group,
    Set,
    Flex,
    FieldStack,
    useModalState,
    Dialog,
    Card,
    Spinner,
    Alert
} from "bumbag"
import { useIsDisplayMenuByRoute } from "../../user/context"
import Disclosure, {
    AddDisclosure,
    ConfigIconDisclosure,
    EditDisclosure
} from "../../dialog/Disclosure"
import {
    useForm,
    useFormContext,
    useFieldArray,
    Controller
} from "react-hook-form"
import FormDialog from "../../form/FormDialog"
import { Trans, useLocale, useTrans } from "../../i18n"
import Backdrop from "../../dialog/Backdrop"
import {
    useUploadGameProviderGames,
    useAddGameProviderGameList
} from "../../game/games/client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import clone from "clone-deep"
import { Table } from "../../table"
import textColumn from "../../table/column/textColumn"
import { InputField, Input } from "../../input/Input"
import { CheckboxField } from "../../checkbox/Checkbox"
import GameStatus, { GameStatusSelectField } from "../../game/games/GameStatus"
import { GameTypeSelectField } from "../../game/GameType"
import { GamePlatformLabel } from "../../game/games/GamePlatform"
import { useDropzone } from "react-dropzone"
import { GameProviderGameFormBody } from "./GameProviderGame"
import { ConfirmText } from "../../button/ActionButtons"
import sequenceColumn from "../../table/column/sequence"
import { Update } from "./menu"

export const BatchAddGameProviderGameListDisclosure = ({
    gameProviderId,
    onSubmitted
}) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([Update])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <Disclosure {...dialog}>
                <Trans zh="批量" en="Batch" />
            </Disclosure>
            {dialog.visible && (
                <BatchAddGameListDialog
                    gameProviderId={gameProviderId}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const GameListFieldName = "games"
const BatchAddGameListDialog = ({ gameProviderId, onSubmitted, dialog }) => {
    let title = useTrans({
        zh: "批量新增游戏",
        en: "Batch Create Games"
    })

    let formMethods = useForm()
    let { control, setValue, watch } = formMethods

    let { fields, append, remove, ...fieldArray } = useFieldArray({
        control,
        name: GameListFieldName,
        keyName: "_id",
        shouldUnregister: true
    })
    let gameListFieldArray = watch(GameListFieldName)
    //TODO: merge by keyName or index is fine?
    let items = fields.map((field, index) => {
        return {
            ...field,
            ...gameListFieldArray[index]
        }
    })

    let [formData, setSubmit] = useState()
    let { isLoading, result, error, mutate, deleteCache } =
        useAddGameProviderGameList(formData)

    let columns = useColumns({ items, formMethods })
    const onSubmit = ({ games = [], ...rest }) => {
        setSubmit({ gameProviderId, games })
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    let uploadRequired = fields.length == 0
    return (
        <Backdrop {...dialog}>
            <FormDialog
                formMethods={formMethods}
                onSubmit={onSubmit}
                title={title}
                isLoading={isLoading}
                result={result}
                error={error}
                onCloseErrorAlert={reset}
                onSubmitted={onSubmitted}
                dialog={dialog}
                hideBackdrop
                width="80vw"
                maxWidth="80vw"
                placement="top"
            >
                {uploadRequired && (
                    <UploadGameListCard
                        gameProviderId={gameProviderId}
                        onUploaded={(uploadedGames = []) => {
                            setValue(GameListFieldName, uploadedGames)
                        }}
                    />
                )}
                {fields.length > 0 && <Table columns={columns} data={items} />}
            </FormDialog>
        </Backdrop>
    )
}

const useColumns = ({ items = [], formMethods }) => {
    return useMemo(() => {
        let columns = [
            textColumn({
                id: "index",
                accessor: (row, rowIndex) => rowIndex + 1,
                Header: <Trans zh="序" en="Sequence" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "externalId",
                Header: <Trans zh="External Id" en="External Id" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "code",
                Header: <Trans zh="游戏代码" en="Code" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "name",
                Header: <Trans zh="游戏名称" en="Name" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "status",
                Header: <Trans zh="Status" en="Status" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "type",
                Header: <Trans zh="Type" en="Type" />,
                disableSortBy: true,
                Cell: ({ value }) => <GamePlatformLabel value={value} />
            }),
            textColumn({
                id: "actions",
                accessor: (row) => row,
                Header: <Trans zh="操作" en="Actions" />,
                disableSortBy: true,
                Cell: ({ value: record, row }) => {
                    let index = row.index
                    return (
                        <Set orientation="horizontal" alignX="center">
                            <EditGameDisclosure
                                record={record}
                                onChange={(game) => {
                                    formMethods?.setValue(
                                        `${GameListFieldName}.${index}`,
                                        game
                                    )
                                }}
                            />
                        </Set>
                    )
                }
            })
        ]

        return columns
    }, [JSON.stringify(items)])
}

const EditGameDisclosure = ({ record, onChange }) => {
    let dialog = useModalState()
    return (
        <>
            <EditDisclosure {...dialog} />
            {dialog.visible && (
                <EditGameDialog
                    record={record}
                    submitText={<ConfirmText />}
                    onChange={onChange}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const EditGameDialog = ({ record, onChange, dialog }) => {
    let title = useTrans({
        zh: "Edit Game",
        en: "Edit Game"
    })

    const onSubmit = (data) => {
        onChange?.(data)
        dialog.hide()
    }

    return (
        <Backdrop {...dialog}>
            <FormDialog
                defaultValues={clone(record)}
                submitText={<ConfirmText />}
                title={title}
                onSubmit={onSubmit}
                stopPropagation
                hideBackdrop
                title={title}
                dialog={dialog}
                width="60vw"
                css={css`
                    & .bb-DialogContent {
                        position: relative;
                    }
                `}
            >
                <GameProviderGameFormBody />
            </FormDialog>
        </Backdrop>
    )
}

const UploadGameListCard = ({ gameProviderId, onUploaded }) => {
    let [formData, setSubmit] = useState()
    const {
        acceptedFiles,
        getRootProps,
        getInputProps,
        isDragActive,
        isDragAccept,
        isDragReject
    } = useDropzone({
        maxFiles: 1,
        accept: ".xlsx",
        onDrop: (files) => {
            if (files.length > 0) {
                let [file] = files
                let formData = new FormData()
                formData.append("id", gameProviderId)
                formData.append("file", file, file.name)
                setSubmit(formData)
            }
        }
    })

    let { isLoading, result, error, deleteCache, mutate } =
        useUploadGameProviderGames(formData)

    useEffect(() => {
        if (result) {
            let games = result?.games?.map((game) => ({
                ...clone(game),
                status: GameStatus.Active
            }))
            onUploaded?.(games)
        }
    }, [result])

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    let notSelectFile = !formData?.file
    return (
        <FieldStack>
            {notSelectFile && (
                <Card
                    {...getRootProps({
                        refKey: "elementRef "
                    })}
                >
                    <input {...getInputProps()} />
                    <p>
                        <Trans zh="选择档案" en="Select File" />
                    </p>
                </Card>
            )}
            {isLoading && (
                <Card alignX="center">
                    <Spinner />
                </Card>
            )}
            {error && (
                <Alert
                    type="danger"
                    title={error?.title ?? "Error"}
                    showCloseButton
                    onClickClose={reset}
                >
                    {JSON.stringify(error)}
                </Alert>
            )}
        </FieldStack>
    )
}
