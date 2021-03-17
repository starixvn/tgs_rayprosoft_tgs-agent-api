import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Box, Flex, Set, Button, Text, Stack } from "bumbag"
import PageContent from "../../page/Content"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserBrandId,
    useUserRoleType
} from "../../user/context"
import { useStationLetterList } from "./client"
import { Code } from "./menu"
import { useTabState, Tabs, TabList, Tab, TabPanel } from "../../tab"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import { Trans } from "../../i18n"
import {
    from as statusFrom,
    Label as StatusLabel,
    InVerify,
    InVerifyName,
    Published,
    PublishedName,
    Rejected,
    RejectedName
} from "./status"
import { AddButton, VerifyButton, ViewButton } from "./edit"
import { Verify as VerifyMenu, View as ViewMenu } from "./menu"
import { useFormContext, useForm, FormProvider } from "react-hook-form"
import { MerchantIdHookFormSelect } from "../../merchant/MerchantSelect"
import { nextRoleType } from "../../system/roles/type"
import BrandBar from "../../report/BrandBar"
import AlertDialog from "../../dialog/Alert"

export { Code }

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

const Body = () => {
    let userBrandId = useUserBrandId()
    let [brandId, setBrandId] = useState(userBrandId)

    return (
        <Set orientation="vertical" isFilled>
            <BrandBar
                onBrandChange={(value) => {
                    setBrandId(value)
                }}
            />
            {brandId && <BodyContent brandId={brandId} />}
        </Set>
    )
}

const BodyContent = ({ brandId }) => {
    let hasVerifyAuth = useIsDisplayMenuByRoute([VerifyMenu])
    let hasViewAuth = useIsDisplayMenuByRoute([ViewMenu])

    let { state, setState, setPageIndex, setSortOrder } = useTable({
        brandId,
        statusName: hasVerifyAuth
            ? InVerifyName
            : hasViewAuth
            ? PublishedName
            : null
    })
    let { pageSize, statusName } = state
    let {
        isLoading,
        data = {},
        error,
        mutate,
        deleteCache
    } = useStationLetterList({
        ...toApiParams(state),
        status: statusFrom(statusName)
    })

    let columns = useColumns({ mutate, statusName })

    let tab = useTabState({
        selectedId: statusName
    })

    const activeTab = (newStatusName) => () => {
        if (newStatusName === statusName) {
            mutate()
        }
        setState({
            pageIndex: 0,
            statusName: newStatusName,
            sortBy: []
        })
    }

    return (
        <PageContent
            title={<Trans zh="站内信" en="Station Letter" />}
            actionBar={
                <Box alignX="right">
                    <AddButton onSubmitted={mutate} />
                </Box>
            }
        >
            <Tabs>
                <TabList {...tab}>
                    {hasVerifyAuth && (
                        <Tab
                            tabId={InVerifyName}
                            onClick={activeTab(InVerifyName)}
                            {...tab}
                        >
                            <StatusLabel value={InVerify} />
                        </Tab>
                    )}
                    <Tab
                        tabId={PublishedName}
                        onClick={activeTab(PublishedName)}
                        {...tab}
                    >
                        <StatusLabel value={Published} />
                    </Tab>
                    <Tab
                        tabId={RejectedName}
                        onClick={activeTab(RejectedName)}
                        {...tab}
                    >
                        <StatusLabel value={Rejected} />
                    </Tab>
                </TabList>
                {hasVerifyAuth && (
                    <TabPanel tabId={InVerifyName} {...tab}>
                        {statusName === InVerifyName && (
                            <Table
                                columns={columns}
                                data={data?.items}
                                pageCount={pageCount(
                                    pageSize,
                                    data?.totalCount
                                )}
                                setPageIndex={setPageIndex}
                                setSortOrder={setSortOrder}
                                initialState={state}
                                isLoading={isLoading}
                            />
                        )}
                    </TabPanel>
                )}

                <TabPanel tabId={PublishedName} {...tab}>
                    {statusName === PublishedName && (
                        <Table
                            columns={columns}
                            data={data?.items}
                            pageCount={pageCount(pageSize, data?.totalCount)}
                            setPageIndex={setPageIndex}
                            setSortOrder={setSortOrder}
                            initialState={state}
                            isLoading={isLoading}
                        />
                    )}
                </TabPanel>

                <TabPanel tabId={RejectedName} {...tab}>
                    {statusName === RejectedName && (
                        <Table
                            columns={columns}
                            data={data?.items}
                            pageCount={pageCount(pageSize, data?.totalCount)}
                            setPageIndex={setPageIndex}
                            setSortOrder={setSortOrder}
                            initialState={state}
                            isLoading={isLoading}
                        />
                    )}
                </TabPanel>
            </Tabs>
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

const useColumns = ({ mutate, statusName }) => {
    let hasVerifyAuth = useIsDisplayMenuByRoute([VerifyMenu])
    let hasViewAuth = useIsDisplayMenuByRoute([ViewMenu])

    return useMemo(() => {
        let columns = [
            sequenceColumn(),
            {
                accessor: "subject",
                Header: <Trans zh="主题" en="Subject" />,
                disableSortBy: true,
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                }
            },
            {
                accessor: "createdByUsername",
                Header: <Trans zh="操作人" en="Operator" />,
                disableSortBy: true,
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                }
            },
            dateTimeColumn({
                accessor: "createdOn",
                Header: <Trans zh="创建时间" en="Created On" />,
                disableSortBy: true
            }),
            {
                id: "status",
                accessor: (record) => record,
                Header: <Trans zh="状态" en="Status" />,
                disableSortBy: true,
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                },
                Cell: ({ value }) => {
                    let { status } = value
                    return (
                        <Text.Block
                            css={css`
                                white-space: nowrap;
                            `}
                        >
                            <StatusLabel value={status} />
                        </Text.Block>
                    )
                }
            }
        ]

        let hasAnyMenu =
            statusName === InVerifyName ? hasVerifyAuth : hasViewAuth

        if (hasAnyMenu) {
            columns.push({
                id: "actions",
                disableSortBy: true,
                Header: <Trans zh="操作" en="Actions" />,
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                },
                accessor: (row) => {
                    return row
                },
                Cell: ({ value }) => {
                    return (
                        <Box alignX="center">
                            <Set>
                                {statusName === InVerifyName && (
                                    <VerifyButton
                                        record={value}
                                        onSubmitted={mutate}
                                    />
                                )}
                                {(statusName === PublishedName ||
                                    statusName === RejectedName) && (
                                    <ViewButton record={value} />
                                )}
                            </Set>
                        </Box>
                    )
                }
            })
        }
        return columns
    }, [statusName])
}
