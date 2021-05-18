import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import {
    Heading,
    Stack,
    Box,
    Button,
    Set,
    useModalState,
    Flex,
    Level
} from "bumbag"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserBrandId
} from "../../user/context"
import { Code } from "./menu"
import {
    FormattedNumber,
    FormattedNumberDecimal,
    Trans,
    useTrans
} from "../../i18n"
import SearchButton from "../../button/SearchButton"
import equals from "fast-deep-equal"
import { InputField } from "../../input/Input"
import AlertDialog from "../../dialog/Alert"
import textColumn from "../../table/column/textColumn"
import numberColumn, {
    numberDecimalColumn,
    numberDecimalPercentageColumn
} from "../../table/column/numberColumn"
import { today } from "../../date/range"
import DateTimeRangeInput from "../../date/DateTimeRangeInput"
import { useLogList } from "./client"
import LogLevel, { LogLevelSelect } from "./LogLevel"
import { LogMessageDialogDisclosure } from "../LogMessageDialog"
import { LogExceptionDialogDisclosure } from "../LogExceptionDialog"

export { Code }

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

export const CodeContentPair = [Code, Content]

const Body = ({}) => {
    let todayRange = today()
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        from: todayRange.start,
        to: todayRange.end,
        trackingId: "",
        serverName: "",
        level: LogLevel.All
    })

    let { pageIndex, pageSize } = state
    let { data = {}, isLoading, mutate, error, deleteCache } = useLogList(
        toApiParams(state)
    )

    let columns = useColumns({ mutate, items: data?.items })

    return (
        <PageContent
            title={<Trans zh="Logs" en="Logs" />}
            actionBar={
                <SearchBar
                    start={state?.from}
                    end={state?.to}
                    trackingId={state?.trackingId}
                    serverName={state?.serverName}
                    level={state?.level}
                    search={({ start, end, ...data }) => {
                        let newState = {
                            pageIndex: 0,
                            from: start,
                            to: end,
                            ...data
                        }
                        if (
                            equals(newState, {
                                pageIndex,
                                from: state?.from,
                                to: state?.to,
                                trackingId: state?.trackingId,
                                serverName: state?.serverName,
                                level: state?.level
                            })
                        ) {
                            mutate()
                        }
                        setState(newState)
                    }}
                    isLoading={isLoading}
                />
            }
        >
            <Table
                controlledState={state}
                columns={columns}
                data={data?.items}
                pageCount={pageCount(pageSize, data?.totalCount)}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                isLoading={isLoading}
            />
            {error && (
                <AlertDialog
                    {...error}
                    onClickClose={() => {
                        deleteCache()
                        mutate({ data: undefined }, false)
                    }}
                />
            )}
        </PageContent>
    )
}

const SearchBar = ({
    start,
    end,
    trackingId,
    serverName,
    level,
    search,
    isLoading,
    ...props
}) => {
    let formMethods = useForm({
        defaultValues: {
            start,
            end,
            trackingId,
            serverName,
            level
        }
    })

    const onSubmit = (data) => search(data)

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                <Set>
                    <DateTimeRangeInput />
                    <InputField
                        name="trackingId"
                        showEmptyFieldValueAddonAfter
                        placeholder="Tracking Id"
                    />
                    <InputField
                        name="serverName"
                        showEmptyFieldValueAddonAfter
                        placeholder="Server Name"
                    />
                    <LogLevelSelect name="level" />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({ items = [] } = {}) => {
    return useMemo(() => {
        let columns = [
            textColumn({
                accessor: "id",
                Header: <Trans zh="Id" en="Id" />,
                disableSortBy: true,
                Cell: ({ value = "" }) => value
            }),
            textColumn({
                accessor: "trackingId",
                Header: <Trans zh="Tracking Id" en="Tracking Id" />,
                disableSortBy: true,
                Cell: ({ value = "" }) => value
            }),
            textColumn({
                accessor: "serverName",
                Header: <Trans zh="Server Name" en="Server Name" />,
                disableSortBy: true,
                Cell: ({ value = "" }) => value
            }),
            textColumn({
                accessor: "level",
                Header: <Trans zh="Level" en="Level" />,
                disableSortBy: true,
                Cell: ({ value = "" }) => value
            }),
            textColumn({
                accessor: "logger",
                Header: <Trans zh="Logger" en="Logger" />,
                disableSortBy: true,
                Cell: ({ value = "" }) => value
            }),
            textColumn({
                accessor: "message",
                Header: <Trans zh="Message" en="Message" />,
                disableSortBy: true,
                Cell: ({ value = "" }) => (
                    <LogMessageDialogDisclosure value={value} />
                )
            }),
            textColumn({
                accessor: "exception",
                Header: <Trans zh="Exception" en="Exception" />,
                disableSortBy: true,
                Cell: ({ value = "" }) => (
                    <LogExceptionDialogDisclosure value={value} />
                )
            }),
            dateTimeColumn({
                accessor: "recordedOn",
                Header: <Trans zh="Recorded On" en="Recorded On" />,
                disableSortBy: true
            })
        ]

        return columns
    }, [JSON.stringify(items)])
}
