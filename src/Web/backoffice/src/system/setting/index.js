import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import { Box, Button, Set, Icon } from "bumbag"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserRoleType,
    useUserRoleId
} from "../../user/context"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Code } from "./menu"
import { useSystemSetting } from "./client"
import textColumn from "../../table/column/textColumn"
import dateTimeColumn from "../../table/column/dateTime"
import { useForm, FormProvider } from "react-hook-form"
import { InputField } from "../../input/Input"
import SearchButton from "../../button/SearchButton"
import { PageContent } from "../../page/Content"
import AlertDialog from "../../dialog/Alert"

export { Code }

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

export const CodeContentPair = [Code, Content]

const Body = () => {
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        key: ""
    })

    let { isLoading, data, error, mutate, deleteCache } = useSystemSetting(
        toApiParams(state)
    )

    let columns = useColumns({ mutate, items: data?.items })
    return (
        <PageContent
            title={<Trans zh="设定" en="Setting" />}
            actionBar={
                <>
                    <SearchBar
                        key={key}
                        search={(data) => {
                            let newState = {
                                pageIndex: 0,
                                key: data?.key
                            }
                            if (equals(newState, { pageIndex, key })) {
                                mutate()
                            }
                            setState(newState)
                        }}
                        isLoading={isLoading}
                    />
                </>
            }
        >
            <Table
                controlledState={state}
                columns={columns}
                data={data?.items}
                pageCount={pageCount(state?.pageSize, data?.totalCount)}
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

const SearchBar = ({ key = "", search, isLoading }) => {
    let formMethods = useForm({
        defaultValues: { key }
    })

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(search)}>
                <Set>
                    <InputField name="key" showEmptyFieldValueAddonAfter />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({ items = [], mutate }) => {
    return useMemo(
        () => [
            textColumn({
                accessor: "key",
                Header: <Trans zh="Key" en="Key" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "description",
                Header: <Trans zh="说明" en="Description" />,
                disableSortBy: true
            }),
            dateTimeColumn({
                Header: <Trans zh="创建时间" en="Created Time" />,
                accessor: "createdOn",
                disableSortBy: true
            }),
            dateTimeColumn({
                Header: <Trans zh="更改时间" en="Updated Time" />,
                accessor: "updatedOn",
                disableSortBy: true
            }),
            textColumn({
                accessor: "createdByUsername",
                Header: <Trans zh="创建人" en="Created By" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "UpdatedByUsername",
                Header: <Trans zh="更新人" en="Updated By" />,
                disableSortBy: true
            }),
            {
                id: "actions",
                accessor: (row) => row,
                Cell: ({ value }) => {
                    return (
                        <Set orientation="horizontal">
                            TODO: edit & delete btn
                        </Set>
                    )
                },
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                }
            }
        ],
        [JSON.stringify(items)]
    )
}
